import { ChangeDetectorRef, Component, NgZone, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';
import { CreateQuestionRequestDto } from '../../Models/Dtos/Question/create-question-request-dto';
import { UpdateQuestionRequestDto } from '../../Models/Dtos/Question/update-question-request-dto';
import {
  CodeCompletionQuestionPayloadDto,
  FillInTheBlankQuestionPayloadDto,
  MultipleChoiceQuestionPayloadDto,
  TrueFalseQuestionPayloadDto,
} from '../../Models/Dtos/Question/question-type-payload-dtos';
import { DifficultyLevel } from '../../Models/Enums/DifficultyLevel';
import { QuestionType } from '../../Models/Enums/QuestionType';
import { AuthService } from '../../services/auth-service';
import { ModalService } from '../../services/modal-service';
import { QuestionBankService } from '../../services/question-bank-service';

@Component({
  selector: 'app-question-bank-form',
  standalone: false,
  templateUrl: './question-bank-form.html',
  styleUrl: './question-bank-form.scss',
})
export class QuestionBankForm implements OnInit {
  form: FormGroup;

  loading = false;
  saving = false;
  isEditMode = false;
  questionId: string | null = null;

  readonly questionTypeOptions = [
    { label: 'Multiple Choice', value: QuestionType.MultipleChoice },
    { label: 'Code Completion', value: QuestionType.CodeCompletion },
    { label: 'True / False', value: QuestionType.TrueFalse },
    { label: 'Fill In The Blank', value: QuestionType.FillInTheBlank },
  ];

  readonly difficultyOptions = [
    { label: 'Junior', value: DifficultyLevel.Junior },
    { label: 'Medior', value: DifficultyLevel.Medior },
    { label: 'Senior', value: DifficultyLevel.Senior },
  ];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private modalService: ModalService,
    private questionBankService: QuestionBankService,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      type: [QuestionType.MultipleChoice, Validators.required],
      language: ['', [Validators.required, Validators.maxLength(20)]],
      difficulty: [DifficultyLevel.Junior, Validators.required],
      title: ['', [Validators.required, Validators.maxLength(255)]],
      questionText: ['', Validators.required],
      isActive: [true],

      multipleChoiceOptions: this.fb.array([
        this.createMultipleChoiceOptionControl(),
        this.createMultipleChoiceOptionControl(),
      ]),
      multipleChoiceCorrectFlags: this.fb.array([
        this.createMultipleChoiceCorrectControl(),
        this.createMultipleChoiceCorrectControl(),
      ]),

      codeSnippet: [''],
      acceptedAnswersText: [''],

      fillInAnswer: [''],
      fillInManualFeedback: [''],

      trueFalseCorrectAnswer: [false],
      trueFalseExplanation: [''],
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!id;
    this.questionId = id;

    if (!id) {
      return;
    }

    this.loading = true;
    this.questionBankService
      .getById(id)
      .pipe(
        finalize(() => {
          this.ngZone.run(() => {
            this.loading = false;
            this.cdr.detectChanges();
          });
        })
      )
      .subscribe({
        next: (question) => {
          this.ngZone.run(() => {
            this.form.patchValue({
              type: question.type,
              language: question.language,
              difficulty: question.difficulty,
              title: question.title,
              questionText: question.questionText,
              isActive: question.isActive,
              codeSnippet: question.codeCompletion?.codeSnippet ?? '',
              acceptedAnswersText: question.codeCompletion?.acceptedAnswers?.join('\n') ?? '',
              fillInAnswer: question.fillInTheBlank?.answer ?? '',
              fillInManualFeedback: question.fillInTheBlank?.manualFeedback ?? '',
              trueFalseCorrectAnswer: question.trueFalse?.correctAnswer ?? false,
              trueFalseExplanation: question.trueFalse?.explanation ?? '',
            });

            this.setMultipleChoiceControls(
              question.multipleChoice?.options ?? [],
              question.multipleChoice?.correctOptionIndexes ?? []
            );

            this.form.get('type')?.disable();
            this.cdr.detectChanges();
          });
        },
        error: () => {
          this.ngZone.run(() => {
            this.cdr.detectChanges();
          });
        },
      });
  }

  get selectedType(): QuestionType {
    const rawType = this.form.getRawValue().type;
    return rawType as QuestionType;
  }

  get multipleChoiceOptionsArray(): FormArray<FormControl<string>> {
    return this.form.get('multipleChoiceOptions') as FormArray<FormControl<string>>;
  }

  get multipleChoiceCorrectFlagsArray(): FormArray<FormControl<boolean>> {
    return this.form.get('multipleChoiceCorrectFlags') as FormArray<FormControl<boolean>>;
  }

  addMultipleChoiceOption(): void {
    this.multipleChoiceOptionsArray.push(this.createMultipleChoiceOptionControl());
    this.multipleChoiceCorrectFlagsArray.push(this.createMultipleChoiceCorrectControl());
  }

  removeMultipleChoiceOption(index: number): void {
    if (this.multipleChoiceOptionsArray.length <= 2) {
      return;
    }

    this.multipleChoiceOptionsArray.removeAt(index);
    this.multipleChoiceCorrectFlagsArray.removeAt(index);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payloadError = this.validateTypePayload();
    if (payloadError) {
      this.modalService.open({
        message: payloadError,
        autoClose: true,
        duration: 3000,
        type: 'error',
      });
      return;
    }

    this.saving = true;

    if (this.isEditMode && this.questionId) {
      const dto = this.buildUpdateDto();
      this.questionBankService
        .update(this.questionId, dto)
        .pipe(
          finalize(() => {
            this.ngZone.run(() => {
              this.saving = false;
              this.cdr.detectChanges();
            });
          })
        )
        .subscribe({
          next: (updated) => {
            this.ngZone.run(() => {
              this.modalService.open({
                message: 'Question updated successfully.',
                autoClose: true,
                duration: 2500,
                type: 'success',
              });
              this.router.navigate(['/question-bank', updated.id]);
            });
          },
        });
      return;
    }

    const createdBy = this.authService.getUserId();
    if (!createdBy) {
      this.saving = false;
      this.modalService.open({
        message: 'Cannot create question: missing user id from token.',
        autoClose: true,
        duration: 3000,
        type: 'error',
      });
      return;
    }

    const dto = this.buildCreateDto(createdBy);
    this.questionBankService
      .create(dto)
      .pipe(
        finalize(() => {
          this.ngZone.run(() => {
            this.saving = false;
            this.cdr.detectChanges();
          });
        })
      )
      .subscribe({
        next: (created) => {
          this.ngZone.run(() => {
            this.modalService.open({
              message: 'Question created successfully.',
              autoClose: true,
              duration: 2500,
              type: 'success',
            });
            this.router.navigate(['/question-bank', created.id]);
          });
        },
      });
  }

  cancel(): void {
    if (this.questionId) {
      this.router.navigate(['/question-bank', this.questionId]);
      return;
    }

    this.router.navigate(['/question-bank']);
  }

  private buildCreateDto(createdBy: string): CreateQuestionRequestDto {
    const value = this.form.getRawValue();
    const dto = new CreateQuestionRequestDto();
    dto.type = value.type as QuestionType;
    dto.language = value.language.trim();
    dto.difficulty = Number(value.difficulty) as DifficultyLevel;
    dto.title = value.title.trim();
    dto.questionText = value.questionText.trim();
    dto.createdBy = createdBy;

    this.attachTypePayload(dto, dto.type);

    return dto;
  }

  private buildUpdateDto(): UpdateQuestionRequestDto {
    const value = this.form.getRawValue();
    const dto = new UpdateQuestionRequestDto();
    const selectedType = value.type as QuestionType;

    dto.language = value.language.trim();
    dto.difficulty = Number(value.difficulty) as DifficultyLevel;
    dto.title = value.title.trim();
    dto.questionText = value.questionText.trim();
    dto.isActive = !!value.isActive;

    this.attachTypePayload(dto, selectedType);

    return dto;
  }

  private attachTypePayload(target: CreateQuestionRequestDto | UpdateQuestionRequestDto, type: QuestionType): void {
    if (type === QuestionType.MultipleChoice) {
      target.multipleChoice = this.buildMultipleChoicePayload();
      return;
    }

    if (type === QuestionType.CodeCompletion) {
      target.codeCompletion = this.buildCodeCompletionPayload();
      return;
    }

    if (type === QuestionType.FillInTheBlank) {
      target.fillInTheBlank = this.buildFillInTheBlankPayload();
      return;
    }

    target.trueFalse = this.buildTrueFalsePayload();
  }

  private buildMultipleChoicePayload(): MultipleChoiceQuestionPayloadDto {
    const payload = new MultipleChoiceQuestionPayloadDto();

    const rawOptions = this.multipleChoiceOptionsArray.controls.map((control) => (control.value ?? '').trim());
    const checkedFlags = this.multipleChoiceCorrectFlagsArray.controls.map((control) => !!control.value);

    const options: string[] = [];
    const correctIndexes: number[] = [];

    rawOptions.forEach((option, originalIndex) => {
      if (option === '') {
        return;
      }

      const targetIndex = options.length;
      options.push(option);

      if (checkedFlags[originalIndex]) {
        correctIndexes.push(targetIndex);
      }
    });

    payload.options = options;
    payload.correctOptionIndexes = correctIndexes;
    payload.allowMultipleSelection = correctIndexes.length > 1;

    return payload;
  }

  private buildCodeCompletionPayload(): CodeCompletionQuestionPayloadDto {
    const value = this.form.getRawValue();
    const payload = new CodeCompletionQuestionPayloadDto();
    payload.codeSnippet = value.codeSnippet.trim();
    payload.acceptedAnswers = this.splitByLine(value.acceptedAnswersText);
    return payload;
  }

  private buildFillInTheBlankPayload(): FillInTheBlankQuestionPayloadDto {
    const value = this.form.getRawValue();
    const payload = new FillInTheBlankQuestionPayloadDto();
    payload.answer = value.fillInAnswer.trim();
    payload.manualFeedback = value.fillInManualFeedback?.trim() || undefined;
    return payload;
  }

  private buildTrueFalsePayload(): TrueFalseQuestionPayloadDto {
    const value = this.form.getRawValue();
    const payload = new TrueFalseQuestionPayloadDto();
    payload.correctAnswer = !!value.trueFalseCorrectAnswer;
    payload.explanation = value.trueFalseExplanation?.trim() || undefined;
    return payload;
  }

  private validateTypePayload(): string | null {
    const type = this.selectedType;
    const value = this.form.getRawValue();

    if (type === QuestionType.MultipleChoice) {
      const payload = this.buildMultipleChoicePayload();

      if (payload.options.length < 2) {
        return 'Multiple choice requires at least two non-empty options.';
      }

      if (payload.correctOptionIndexes.length === 0) {
        return 'Multiple choice requires at least one checked correct answer.';
      }

      return null;
    }

    if (type === QuestionType.CodeCompletion) {
      if (!value.codeSnippet || value.codeSnippet.trim() === '') {
        return 'Code completion requires a code snippet.';
      }

      if (this.splitByLine(value.acceptedAnswersText).length === 0) {
        return 'Code completion requires at least one accepted answer.';
      }

      return null;
    }

    if (type === QuestionType.FillInTheBlank) {
      if (!value.fillInAnswer || value.fillInAnswer.trim() === '') {
        return 'Fill in the blank requires an answer.';
      }

      return null;
    }

    return null;
  }

  private splitByLine(raw: string): string[] {
    return (raw ?? '')
      .split(/\r?\n/)
      .map((item) => item.trim())
      .filter((item) => item !== '');
  }

  private createMultipleChoiceOptionControl(value = ''): FormControl<string> {
    return this.fb.control(value, { nonNullable: true });
  }

  private createMultipleChoiceCorrectControl(checked = false): FormControl<boolean> {
    return this.fb.control(checked, { nonNullable: true });
  }

  private setMultipleChoiceControls(options: string[], correctIndexes: number[]): void {
    this.multipleChoiceOptionsArray.clear();
    this.multipleChoiceCorrectFlagsArray.clear();

    options.forEach((option, index) => {
      this.multipleChoiceOptionsArray.push(this.createMultipleChoiceOptionControl(option));
      this.multipleChoiceCorrectFlagsArray.push(
        this.createMultipleChoiceCorrectControl(correctIndexes.includes(index))
      );
    });

    while (this.multipleChoiceOptionsArray.length < 2) {
      this.multipleChoiceOptionsArray.push(this.createMultipleChoiceOptionControl());
      this.multipleChoiceCorrectFlagsArray.push(this.createMultipleChoiceCorrectControl());
    }
  }
}
