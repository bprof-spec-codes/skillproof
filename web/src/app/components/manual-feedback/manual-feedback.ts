import { Component, OnInit } from '@angular/core';
import { UserTestsDto } from '../../Models/Dtos/User/userTests-dto';
import { FeedbackShared } from '../../services/feedback-shared';
import { TestService } from '../../services/test-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-manual-feedback',
  standalone: false,
  templateUrl: './manual-feedback.html',
  styleUrl: './manual-feedback.scss',
})
export class ManualFeedback implements OnInit {
  testQuestionId: string | null = null;
  score: number | null = null;
  testAnswerId: string | null = null;
  feedback: string = '';

  testQuestionData: UserTestsDto | null = null;
  userData: any;

  constructor(
    private feedbackSharedService: FeedbackShared,
    private testService: TestService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.testQuestionData = this.feedbackSharedService.selectedQuestion
    this.userData = this.feedbackSharedService.selectedUser

    if(this.testQuestionData) {
      this.testQuestionId = this.testQuestionData.questionId;
      this.testAnswerId = this.testQuestionData.testAnswerId;
    }
  }

  submitManualFeedback(): void {
    if (this.testAnswerId && this.score != null) {
      this.testService.manualFeedback(this.feedback, this.score, this.testAnswerId).subscribe({
        next: (data) => {
          console.log('Feedback successfully added', data);
          
          const jobId = this.feedbackSharedService.jobId;

          this.router.navigate(['/reviewUser', this.userData.id], {
            queryParams: { jobId: jobId }
          });
        },
        error: (err) => {
          console.error('Failed to update the scoring', err);
        }
      });
      return;
    }
  }
}
