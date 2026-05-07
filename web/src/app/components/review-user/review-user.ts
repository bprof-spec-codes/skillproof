import { Component, OnInit, ChangeDetectorRef } from '@angular/core'; // <-- 1. Import it
import { ProfileViewDto } from '../../Models/User/profile-view-dto';
import { UserTestsDto } from '../../Models/Dtos/User/userTests-dto';
import { ActivatedRoute, Router } from '@angular/router';
import { ProfileService } from '../../services/profile-service';
import { TestService } from '../../services/test-service';
import { QuestionType } from '../../Models/Enums/QuestionType';
import { combineLatest, of } from 'rxjs';
import { QuestionBankService } from '../../services/question-bank-service';
import { FeedbackShared } from '../../services/feedback-shared';

@Component({
  selector: 'app-review-user',
  standalone: false,
  templateUrl: './review-user.html',
  styleUrl: './review-user.scss',
})
export class ReviewUser implements OnInit {
  userId: string | null = null;
  jobId: string | null = null;

  profileData: ProfileViewDto | null = null;
  testQuestionsData: UserTestsDto[] | null = null;

  public QuestionType = QuestionType;

  constructor(
    private route: ActivatedRoute,
    private profileService: ProfileService,
    private testService: TestService,
    private cdr: ChangeDetectorRef,
    private questionBankService: QuestionBankService,
    private feedbackSharedService: FeedbackShared,
    private router: Router
  ) { }

 ngOnInit(): void {
    combineLatest([
      this.route.paramMap,
      this.route.queryParamMap
    ]).subscribe(([params, queryParams]) => {
      
      this.userId = params.get('id');
      this.jobId = queryParams.get('jobId');

      if (this.userId) {
        this.profileService.getProfile(this.userId).subscribe({
          next: (profile) => {
            this.profileData = profile;
            this.cdr.detectChanges(); 
          },
          error: (err) => console.error('Error fetching the profile', err)
        });
      }

      if (this.userId && this.jobId) {
        this.testService.getUserTestQuestions(this.userId, this.jobId).subscribe({
          next: (data) => {
            this.processAnswers(data);
          },
          error: (err) => console.error('error fetching the user answers', err)
        });
      }

    });
  }

  private processAnswers(answers: UserTestsDto[]): void {
    const tasks = answers.map(answer => {
      if (answer.questionType === QuestionType.MultipleChoice && answer.questionId) {
        
        return this.questionBankService.getById(answer.questionId).pipe(
        );
      }
      return of(answer);
    });

    let countCompletions = 0;
    
    if(answers.length === 0) {
      this.testQuestionsData = answers;
      this.cdr.detectChanges();
      return;
    }

    answers.forEach(answer => {
      if (answer.questionType === QuestionType.MultipleChoice && answer.questionId) {
        this.questionBankService.getById(answer.questionId).subscribe({
          next: (questionDetails) => {
            try {
              const chosenIndexes = JSON.parse(answer.userResponse); 
              
              if (Array.isArray(chosenIndexes) && questionDetails.multipleChoice?.options) {
                const texts = chosenIndexes.map((mappedIndex: number) => 
                  questionDetails.multipleChoice!.options[mappedIndex]
                );
                
                answer.userResponse = texts.join(', ');
              }
            } catch (e) {
              console.error("Could not parse Multiple Choice indexes", e);
            }
            
            this.checkAllFinished(answers, ++countCompletions);
          },
          error: () => this.checkAllFinished(answers, ++countCompletions)
        });
      } else {
        this.checkAllFinished(answers, ++countCompletions);
      }
    });
  }

  private checkAllFinished(answers: UserTestsDto[], completions: number) {
    if (completions === answers.length) {
      this.testQuestionsData = answers;
      this.cdr.detectChanges(); 
    }
  }

  goToManualFeedback(question: UserTestsDto) {
    this.feedbackSharedService.selectedQuestion = question;
    this.feedbackSharedService.selectedUser = this.profileData as any;
    this.feedbackSharedService.jobId = this.jobId
    this.router.navigate(['/manualFeedback', question.questionId])
  }
}