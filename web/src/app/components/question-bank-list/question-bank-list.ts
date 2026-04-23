import { ChangeDetectorRef, Component, HostListener, NgZone, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { QuestionResponseDto } from '../../Models/Dtos/Question/question-response-dto';
import { DifficultyLevel } from '../../Models/Enums/DifficultyLevel';
import { QuestionType } from '../../Models/Enums/QuestionType';
import { QuestionBankService } from '../../services/question-bank-service';
import { ModalService } from '../../services/modal-service';

@Component({
  selector: 'app-question-bank-list',
  standalone: false,
  templateUrl: './question-bank-list.html',
  styleUrl: './question-bank-list.scss',
})
export class QuestionBankList implements OnInit {
  allQuestions: QuestionResponseDto[] = [];
  questions: QuestionResponseDto[] = [];
  availableTags: string[] = [];
  loading = false;

  filterType = '';
  filterDifficulty = '';
  filterIsActive = '';

  tagQuery = '';
  selectedTags: string[] = [];
  showTagDropdown = false;
  groupByTagsEnabled = false;
  collapsedGroups: Record<string, boolean> = {};

  readonly questionTypeOptions = [
    { label: 'Multiple Choice', value: QuestionType.MultipleChoice },
    { label: 'Code Completion', value: QuestionType.CodeCompletion },
    { label: 'True / False', value: QuestionType.TrueFalse },
    { label: 'Open-Ended', value: QuestionType.OpenEnded },
  ];

  readonly difficultyOptions = [
    { label: 'Junior', value: DifficultyLevel.Junior },
    { label: 'Medior', value: DifficultyLevel.Medior },
    { label: 'Senior', value: DifficultyLevel.Senior },
  ];

  constructor(
    private questionBankService: QuestionBankService,
    private modalService: ModalService,
    private router: Router,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadQuestions();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement | null;
    if (!target || target.closest('.tag-filter')) {
      return;
    }

    this.showTagDropdown = false;
  }

  get filteredTagSuggestions(): string[] {
    const query = this.normalizeTag(this.tagQuery);
    return this.availableTags
      .filter((tag) => !this.selectedTags.some((selected) => this.normalizeTag(selected) === this.normalizeTag(tag)))
      .filter((tag) => query === '' || this.normalizeTag(tag).includes(query))
      .slice(0, 12);
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  toggleGroupByTags(): void {
    this.groupByTagsEnabled = !this.groupByTagsEnabled;
    if (this.groupByTagsEnabled) {
      this.syncCollapsedGroups();
    }
  }

  get groupedQuestionsByTag(): Array<{ tag: string; count: number; questions: QuestionResponseDto[]; key: string }> {
    const buckets = new Map<string, { tag: string; questions: QuestionResponseDto[]; key: string }>();
    const untaggedQuestions: QuestionResponseDto[] = [];

    this.questions.forEach((question) => {
      const uniqueTagsForQuestion = Array.from(
        new Set((question.tags ?? []).map((tag) => tag.trim()).filter((tag) => tag.length > 0))
      );

      if (uniqueTagsForQuestion.length === 0) {
        untaggedQuestions.push(question);
        return;
      }

      uniqueTagsForQuestion.forEach((rawTag) => {
        const key = this.normalizeTag(rawTag);
        const existing = buckets.get(key);
        if (existing) {
          existing.questions.push(question);
          return;
        }

        buckets.set(key, {
          key,
          tag: rawTag,
          questions: [question],
        });
      });
    });

    if (untaggedQuestions.length > 0) {
      buckets.set('__untagged__', {
        key: '__untagged__',
        tag: 'No tags',
        questions: untaggedQuestions,
      });
    }

    return Array.from(buckets.values())
      .map((group) => ({
        ...group,
        count: group.questions.length,
      }))
      .sort((a, b) => {
        if (b.count !== a.count) {
          return b.count - a.count;
        }
        return a.tag.localeCompare(b.tag);
      });
  }

  isGroupCollapsed(groupKey: string): boolean {
    return this.collapsedGroups[groupKey] ?? true;
  }

  toggleGroup(groupKey: string): void {
    this.collapsedGroups[groupKey] = !this.isGroupCollapsed(groupKey);
  }

  onTagInputFocus(): void {
    this.showTagDropdown = true;
  }

  onTagInputChange(value: string): void {
    this.tagQuery = value;
    this.showTagDropdown = true;
  }

  onTagInputKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      const [firstSuggestion] = this.filteredTagSuggestions;
      if (firstSuggestion) {
        this.selectTag(firstSuggestion);
      }
      return;
    }

    if (event.key === 'Backspace' && this.tagQuery.trim() === '' && this.selectedTags.length > 0) {
      this.removeTag(this.selectedTags[this.selectedTags.length - 1]);
      return;
    }

    if (event.key === 'Escape') {
      this.showTagDropdown = false;
    }
  }

  selectTag(tag: string): void {
    const normalized = this.normalizeTag(tag);
    if (normalized === '') {
      return;
    }

    const alreadySelected = this.selectedTags.some((selected) => this.normalizeTag(selected) === normalized);
    if (alreadySelected) {
      this.tagQuery = '';
      this.showTagDropdown = false;
      return;
    }

    this.selectedTags = [...this.selectedTags, tag.trim()];
    this.tagQuery = '';
    this.showTagDropdown = true;
    this.applyFilters();
  }

  removeTag(tag: string): void {
    const normalized = this.normalizeTag(tag);
    this.selectedTags = this.selectedTags.filter((selected) => this.normalizeTag(selected) !== normalized);
    this.applyFilters();
  }

  clearFilters(): void {
    this.filterType = '';
    this.filterDifficulty = '';
    this.filterIsActive = '';
    this.tagQuery = '';
    this.selectedTags = [];
    this.showTagDropdown = false;
    this.applyFilters();
  }

  goToCreate(): void {
    this.router.navigate(['/question-bank/create']);
  }

  viewQuestion(id: string): void {
    this.router.navigate(['/question-bank', id]);
  }

  editQuestion(id: string): void {
    this.router.navigate(['/question-bank', id, 'edit']);
  }

  deleteQuestion(id: string): void {
    const shouldDelete = window.confirm('Are you sure you want to delete this question?');
    if (!shouldDelete) {
      return;
    }

    this.questionBankService.delete(id).subscribe({
      next: () => {
        this.allQuestions = this.allQuestions.filter((q) => q.id !== id);
        this.rebuildAvailableTags();
        this.applyFilters();

        this.modalService.open({
          message: 'Question deleted successfully.',
          autoClose: true,
          duration: 2500,
          type: 'success',
        });
      },
    });
  }

  getQuestionTypeLabel(type: QuestionType): string {
    return this.questionTypeOptions.find((option) => option.value === type)?.label ?? String(type);
  }

  getDifficultyLabel(difficulty: DifficultyLevel): string {
    return this.difficultyOptions.find((option) => option.value === difficulty)?.label ?? String(difficulty);
  }

  private loadQuestions(): void {
    this.loading = true;

    this.questionBankService
      .getAll()
      .pipe(
        finalize(() => {
          this.ngZone.run(() => {
            this.loading = false;
            this.cdr.detectChanges();
          });
        })
      )
      .subscribe({
        next: (questions) => {
          this.ngZone.run(() => {
            this.allQuestions = questions;
            this.rebuildAvailableTags();
            this.applyFilters();
            this.cdr.detectChanges();
          });
        },
        error: () => {
          this.ngZone.run(() => {
            this.allQuestions = [];
            this.questions = [];
            this.availableTags = [];
            this.cdr.detectChanges();
          });
        },
      });
  }

  private applyFilters(): void {
    const selectedType = this.filterType.trim();
    const selectedDifficulty = this.filterDifficulty.trim();
    const selectedStatus = this.filterIsActive.trim();
    const normalizedSelectedTags = this.selectedTags.map((tag) => this.normalizeTag(tag));

    this.questions = this.allQuestions.filter((question) => {
      if (selectedType !== '' && question.type !== selectedType) {
        return false;
      }

      if (selectedDifficulty !== '' && String(question.difficulty) !== selectedDifficulty) {
        return false;
      }

      if (selectedStatus !== '' && String(question.isActive) !== selectedStatus) {
        return false;
      }

      if (normalizedSelectedTags.length > 0) {
        const questionTagSet = new Set((question.tags ?? []).map((tag) => this.normalizeTag(tag)));
        const hasAllTags = normalizedSelectedTags.every((tag) => questionTagSet.has(tag));
        if (!hasAllTags) {
          return false;
        }
      }

      return true;
    });

    if (this.groupByTagsEnabled) {
      this.syncCollapsedGroups();
    }
  }

  private rebuildAvailableTags(): void {
    const byNormalizedTag = new Map<string, string>();

    this.allQuestions.forEach((question) => {
      (question.tags ?? []).forEach((tag) => {
        const trimmed = tag.trim();
        const normalized = this.normalizeTag(trimmed);
        if (normalized === '' || byNormalizedTag.has(normalized)) {
          return;
        }

        byNormalizedTag.set(normalized, trimmed);
      });
    });

    this.availableTags = Array.from(byNormalizedTag.values()).sort((a, b) => a.localeCompare(b));
  }

  private normalizeTag(value: string): string {
    return value.trim().toLocaleLowerCase();
  }

  private syncCollapsedGroups(): void {
    const next: Record<string, boolean> = {};
    this.groupedQuestionsByTag.forEach((group) => {
      next[group.key] = this.collapsedGroups[group.key] ?? true;
    });
    this.collapsedGroups = next;
  }
}
