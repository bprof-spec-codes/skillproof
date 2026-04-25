import { Component, HostListener, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { QuestionResponseDto } from '../../Models/Dtos/Question/question-response-dto';
import { DifficultyLevel } from '../../Models/Enums/DifficultyLevel';
import { QuestionType } from '../../Models/Enums/QuestionType';
import { QuestionBankService } from '../../services/question-bank-service';
import { AssessmentService } from '../../services/assesmentservice';

interface RandomTagRule {
  tag: string;
  count: number;
}

interface TagGroupSummary {
  tag: string;
  key: string;
  total: number;
  available: number;
  reserved: number;
}

interface ResolvedRandomRule {
  rule: RandomTagRule;
  questions: QuestionResponseDto[];
}

@Component({
  selector: 'app-assessment-create',
  standalone: false,
  templateUrl: './assessment-create.html',
  styleUrl: './assessment-create.scss',
})
export class AssessmentCreate implements OnInit {
  title = '';
  description = '';
  difficultyLevel = 0;

  availableQuestions: QuestionResponseDto[] = [];
  filteredQuestions: QuestionResponseDto[] = [];
  availableTags: string[] = [];
  selectedQuestions: QuestionResponseDto[] = [];
  randomTagRules: RandomTagRule[] = [];

  showQuestionModal = false;
  loading = false;
  questionsLoading = false;
  error = '';
  questionPickerError = '';

  filterType = '';
  filterDifficulty = '';
  tagQuery = '';
  selectedFilterTags: string[] = [];
  showTagDropdown = false;
  randomGroupDraftCounts: Record<string, number> = {};
  randomPreviewQuestionIds: Record<string, string[]> = {};

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
    private assessmentService: AssessmentService,
    private questionBankService: QuestionBankService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loadActiveQuestions();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement | null;
    if (!target || target.closest('.ac-tag-filter')) {
      return;
    }

    this.showTagDropdown = false;
  }

  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    if (this.showQuestionModal) {
      this.closeQuestionModal();
    }
  }

  get filteredTagSuggestions(): string[] {
    const query = this.normalizeTag(this.tagQuery);
    return this.availableTags
      .filter((tag) => !this.selectedFilterTags.some((selected) => this.normalizeTag(selected) === this.normalizeTag(tag)))
      .filter((tag) => query === '' || this.normalizeTag(tag).includes(query))
      .slice(0, 12);
  }

  get tagGroups(): TagGroupSummary[] {
    const groups = new Map<string, { tag: string; questions: QuestionResponseDto[] }>();
    const normalizedSelectedTags = new Set(this.selectedFilterTags.map((tag) => this.normalizeTag(tag)));
    const reservedQuestionIds = new Set(
      this.resolveRandomRules(false).flatMap((entry) => entry.questions.map((question) => question.id))
    );

    this.targetDifficultyQuestions.forEach((question) => {
      this.getUniqueTags(question).forEach((tag) => {
        const key = this.normalizeTag(tag);
        if (normalizedSelectedTags.size > 0 && !normalizedSelectedTags.has(key)) {
          return;
        }

        const existing = groups.get(key);
        if (existing) {
          existing.questions.push(question);
          return;
        }

        groups.set(key, { tag, questions: [question] });
      });
    });

    return Array.from(groups.entries())
      .map(([key, group]) => {
        const selectedIds = new Set(this.selectedQuestions.map((question) => question.id));
        const total = group.questions.length;
        const available = group.questions.filter(
          (question) => !selectedIds.has(question.id) && !reservedQuestionIds.has(question.id)
        ).length;
        const reserved = this.randomTagRules
          .filter((rule) => this.normalizeTag(rule.tag) === key)
          .reduce((sum, rule) => sum + rule.count, 0);

        return {
          tag: group.tag,
          key,
          total,
          available,
          reserved,
        };
      })
      .sort((a, b) => {
        if (b.total !== a.total) {
          return b.total - a.total;
        }
        return a.tag.localeCompare(b.tag);
      });
  }

  get totalQuestionCount(): number {
    return this.selectedQuestions.length + this.randomTagRules.reduce((sum, rule) => sum + rule.count, 0);
  }

  get hasAssessmentQuestions(): boolean {
    return this.totalQuestionCount > 0;
  }

  private get targetDifficultyQuestions(): QuestionResponseDto[] {
    const targetDifficulty = String(this.difficultyLevel);
    return this.availableQuestions.filter((question) => String(question.difficulty) === targetDifficulty);
  }

  onTargetDifficultyChange(): void {
    this.selectedQuestions = this.selectedQuestions.filter(
      (question) => String(question.difficulty) === String(this.difficultyLevel)
    );
    this.rebuildAvailableTags();
    this.applyFilters();
    this.clampRandomRules();
    this.refreshRandomPreview();
    this.questionPickerError = '';
  }

  onModalDialogClick(event: MouseEvent): void {
    event.stopPropagation();
    const target = event.target as HTMLElement | null;
    if (!target || target.closest('.ac-tag-filter')) {
      return;
    }

    this.showTagDropdown = false;
  }

  openQuestionModal(): void {
    this.questionPickerError = '';
    this.showQuestionModal = true;
  }

  closeQuestionModal(): void {
    this.showQuestionModal = false;
    this.showTagDropdown = false;
  }

  addQuestion(question: QuestionResponseDto): void {
    if (this.isQuestionSelected(question.id)) {
      return;
    }

    this.selectedQuestions = [...this.selectedQuestions, question];
    this.questionPickerError = '';
    this.clampRandomRules();
    this.refreshRandomPreview();
  }

  removeQuestion(id: string): void {
    this.selectedQuestions = this.selectedQuestions.filter((q) => q.id !== id);
    this.questionPickerError = '';
    this.refreshRandomPreview();
  }

  toggleQuestion(question: QuestionResponseDto): void {
    if (this.isQuestionSelected(question.id)) {
      this.removeQuestion(question.id);
      return;
    }

    this.addQuestion(question);
  }

  isQuestionSelected(id: string): boolean {
    return this.selectedQuestions.some((question) => question.id === id);
  }

  addRandomRule(tag: string, requestedCount?: string | number): void {
    const group = this.getTagGroup(tag);
    if (!group || group.available <= 0) {
      this.questionPickerError = `There are no more available questions with the "${tag}" tag.`;
      return;
    }

    const parsedCount = requestedCount === undefined ? 1 : Math.floor(Number(requestedCount));
    const countToAdd = Math.max(1, Math.min(Number.isFinite(parsedCount) ? parsedCount : 1, group.available));
    const existing = this.randomTagRules.find((rule) => this.normalizeTag(rule.tag) === this.normalizeTag(tag));
    if (existing) {
      this.updateRandomRuleCount(existing.tag, existing.count + countToAdd);
      return;
    }

    this.randomTagRules = [...this.randomTagRules, { tag, count: countToAdd }];
    this.setRandomGroupDraftCount(tag, 1);
    this.questionPickerError = '';
    this.refreshRandomPreview();
  }

  updateRandomRuleCount(tag: string, value: string | number): void {
    const numericValue = Number(value);
    const nextCount = Number.isFinite(numericValue) ? Math.floor(numericValue) : 0;
    const currentRule = this.randomTagRules.find((rule) => this.normalizeTag(rule.tag) === this.normalizeTag(tag));
    const currentCount = currentRule?.count ?? 0;
    const group = this.getTagGroup(tag);
    const maxCount = (group?.available ?? 0) + currentCount;
    const clampedCount = Math.max(0, Math.min(nextCount, maxCount));

    if (clampedCount === 0) {
      this.removeRandomRule(tag);
      return;
    }

    this.randomTagRules = this.randomTagRules.map((rule) =>
      this.normalizeTag(rule.tag) === this.normalizeTag(tag) ? { ...rule, count: clampedCount } : rule
    );
    this.clampRandomRules();
    this.refreshRandomPreview();
    this.questionPickerError = '';
  }

  removeRandomRule(tag: string): void {
    const normalizedTag = this.normalizeTag(tag);
    this.randomTagRules = this.randomTagRules.filter((rule) => this.normalizeTag(rule.tag) !== normalizedTag);
    const { [normalizedTag]: _removed, ...nextPreview } = this.randomPreviewQuestionIds;
    this.randomPreviewQuestionIds = nextPreview;
    this.questionPickerError = '';
    this.refreshRandomPreview();
  }

  getRuleMaxCount(tag: string): number {
    return this.getAvailableQuestionCountForRule(tag);
  }

  getTagAvailableCount(tag: string): number {
    return this.getTagGroup(tag)?.available ?? 0;
  }

  getTagTotalCount(tag: string): number {
    return this.getTagGroup(tag)?.total ?? 0;
  }

  getRandomGroupDraftCount(tag: string): number {
    const key = this.normalizeTag(tag);
    const group = this.getTagGroup(tag);
    const value = this.randomGroupDraftCounts[key] ?? 1;
    return Math.max(1, Math.min(value, Math.max(group?.available ?? 1, 1)));
  }

  setRandomGroupDraftCount(tag: string, value: string | number): void {
    const key = this.normalizeTag(tag);
    const group = this.getTagGroup(tag);
    const parsedValue = Math.floor(Number(value));
    const max = Math.max(group?.available ?? 1, 1);
    this.randomGroupDraftCounts = {
      ...this.randomGroupDraftCounts,
      [key]: Math.max(1, Math.min(Number.isFinite(parsedValue) ? parsedValue : 1, max)),
    };
  }

  isQuestionInRandomPreview(questionId: string): boolean {
    return Object.values(this.randomPreviewQuestionIds).some((ids) => ids.includes(questionId));
  }

  getRandomPreviewLabel(questionId: string): string {
    const rule = this.randomTagRules.find((entry) =>
      (this.randomPreviewQuestionIds[this.normalizeTag(entry.tag)] ?? []).includes(questionId)
    );
    return rule ? `Random: ${rule.tag}` : '';
  }

  onFilterChange(): void {
    this.applyFilters();
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
        this.selectFilterTag(firstSuggestion);
      }
      return;
    }

    if (event.key === 'Backspace' && this.tagQuery.trim() === '' && this.selectedFilterTags.length > 0) {
      this.removeFilterTag(this.selectedFilterTags[this.selectedFilterTags.length - 1]);
      return;
    }

    if (event.key === 'Escape') {
      this.showTagDropdown = false;
    }
  }

  selectFilterTag(tag: string): void {
    const normalizedTag = this.normalizeTag(tag);
    if (normalizedTag === '') {
      return;
    }

    const alreadySelected = this.selectedFilterTags.some((selected) => this.normalizeTag(selected) === normalizedTag);
    if (!alreadySelected) {
      this.selectedFilterTags = [...this.selectedFilterTags, tag.trim()];
    }

    this.tagQuery = '';
    this.showTagDropdown = true;
    this.applyFilters();
  }

  removeFilterTag(tag: string): void {
    const normalizedTag = this.normalizeTag(tag);
    this.selectedFilterTags = this.selectedFilterTags.filter((selected) => this.normalizeTag(selected) !== normalizedTag);
    this.applyFilters();
  }

  clearFilters(): void {
    this.filterType = '';
    this.filterDifficulty = '';
    this.tagQuery = '';
    this.selectedFilterTags = [];
    this.showTagDropdown = false;
    this.applyFilters();
  }

  getQuestionTypeLabel(type: QuestionType): string {
    return this.questionTypeOptions.find((option) => option.value === type)?.label ?? String(type);
  }

  getDifficultyLabel(difficulty: DifficultyLevel): string {
    return this.difficultyOptions.find((option) => option.value === difficulty)?.label ?? String(difficulty);
  }

  getQuestionPreview(question: QuestionResponseDto): string {
    const text = question.questionText?.trim();
    return text ? text : 'No question text provided.';
  }

  onSubmit(): void {
    this.error = '';

    if (!this.title.trim()) {
      this.error = 'Please provide a title.';
      return;
    }

    if (!this.hasAssessmentQuestions) {
      this.error = 'Please select at least one fixed question or random tag group.';
      return;
    }

    const resolved = this.resolveQuestionSelection();
    if (!resolved) {
      return;
    }

    this.loading = true;
    const dto = {
      title: this.title,
      description: this.description,
      difficultyLevel: Number(this.difficultyLevel),
      questionIds: resolved,
    };

    this.assessmentService.createAssessment(dto).subscribe({
      next: () => {
        this.router.navigate(['/assessments']);
      },
      error: () => {
        this.error = 'Failed to create the assessment template.';
        this.loading = false;
      },
    });
  }

  previewRandomRule(rule: RandomTagRule): QuestionResponseDto[] {
    const resolved = this.resolveRandomRules(false);
    return resolved.find((entry) => this.normalizeTag(entry.rule.tag) === this.normalizeTag(rule.tag))?.questions ?? [];
  }

  private loadActiveQuestions(): void {
    this.questionsLoading = true;

    this.questionBankService.getAll({ isActive: true }).subscribe({
      next: (questions) => {
        this.availableQuestions = questions.filter((question) => question.isActive);
        this.rebuildAvailableTags();
        this.applyFilters();
        this.clampRandomRules();
        this.refreshRandomPreview();
        this.questionsLoading = false;
      },
      error: () => {
        this.availableQuestions = [];
        this.filteredQuestions = [];
        this.availableTags = [];
        this.questionsLoading = false;
        this.error = 'Failed to load active questions.';
      },
    });
  }

  private applyFilters(): void {
    const selectedType = this.filterType.trim();
    const selectedDifficulty = this.filterDifficulty.trim();
    const normalizedSelectedTags = this.selectedFilterTags.map((tag) => this.normalizeTag(tag));

    this.filteredQuestions = this.targetDifficultyQuestions.filter((question) => {
      if (selectedType !== '' && question.type !== selectedType) {
        return false;
      }

      if (selectedDifficulty !== '' && String(question.difficulty) !== selectedDifficulty) {
        return false;
      }

      if (normalizedSelectedTags.length > 0) {
        const questionTagSet = new Set(this.getUniqueTags(question).map((tag) => this.normalizeTag(tag)));
        const hasAllTags = normalizedSelectedTags.every((tag) => questionTagSet.has(tag));
        if (!hasAllTags) {
          return false;
        }
      }

      return true;
    });
  }

  private rebuildAvailableTags(): void {
    const tagsByNormalizedValue = new Map<string, string>();

    this.targetDifficultyQuestions.forEach((question) => {
      this.getUniqueTags(question).forEach((tag) => {
        const normalizedTag = this.normalizeTag(tag);
        if (normalizedTag === '' || tagsByNormalizedValue.has(normalizedTag)) {
          return;
        }

        tagsByNormalizedValue.set(normalizedTag, tag);
      });
    });

    this.availableTags = Array.from(tagsByNormalizedValue.values()).sort((a, b) => a.localeCompare(b));
  }

  private resolveQuestionSelection(): string[] | null {
    const selectedIds = this.selectedQuestions.map((question) => question.id);
    const usedIds = new Set(selectedIds);
    const resolvedRandomRules = this.resolveRandomRulesFromPreview();

    for (const entry of resolvedRandomRules) {
      if (entry.questions.length !== entry.rule.count) {
        this.error = `The "${entry.rule.tag}" random group needs ${entry.rule.count} questions, but only ${entry.questions.length} are available.`;
        return null;
      }

      entry.questions.forEach((question) => usedIds.add(question.id));
    }

    return Array.from(usedIds);
  }

  private resolveRandomRules(randomize: boolean): ResolvedRandomRule[] {
    const usedIds = new Set(this.selectedQuestions.map((question) => question.id));

    return this.randomTagRules.map((rule) => {
      const candidates = this.targetDifficultyQuestions.filter((question) => {
        if (usedIds.has(question.id)) {
          return false;
        }

        return this.getUniqueTags(question).some((tag) => this.normalizeTag(tag) === this.normalizeTag(rule.tag));
      });

      const orderedCandidates = randomize ? this.shuffleQuestions(candidates) : candidates;
      const questions = orderedCandidates.slice(0, rule.count);
      questions.forEach((question) => usedIds.add(question.id));

      return { rule, questions };
    });
  }

  private clampRandomRules(): void {
    const usedIds = new Set(this.selectedQuestions.map((question) => question.id));
    const nextRules: RandomTagRule[] = [];

    this.randomTagRules.forEach((rule) => {
      const candidates = this.getQuestionsByTag(rule.tag).filter((question) => !usedIds.has(question.id));
      const count = Math.min(rule.count, candidates.length);
      if (count <= 0) {
        return;
      }

      candidates.slice(0, count).forEach((question) => usedIds.add(question.id));
      nextRules.push({ ...rule, count });
    });

    this.randomTagRules = nextRules;
  }

  private refreshRandomPreview(): void {
    const usedIds = new Set(this.selectedQuestions.map((question) => question.id));
    const nextPreview: Record<string, string[]> = {};

    this.randomTagRules.forEach((rule) => {
      const key = this.normalizeTag(rule.tag);
      const existingIds = this.randomPreviewQuestionIds[key] ?? [];
      const candidates = this.getQuestionsByTag(rule.tag).filter((question) => !usedIds.has(question.id));
      const candidateIds = new Set(candidates.map((question) => question.id));
      const keptIds = existingIds.filter((id) => candidateIds.has(id)).slice(0, rule.count);
      const keptIdSet = new Set(keptIds);
      const missingCount = rule.count - keptIds.length;
      const fillIds = this.shuffleQuestions(candidates.filter((question) => !keptIdSet.has(question.id)))
        .slice(0, missingCount)
        .map((question) => question.id);

      nextPreview[key] = [...keptIds, ...fillIds];
      nextPreview[key].forEach((id) => usedIds.add(id));
    });

    this.randomPreviewQuestionIds = nextPreview;
  }

  private resolveRandomRulesFromPreview(): ResolvedRandomRule[] {
    return this.randomTagRules.map((rule) => {
      const ids = this.randomPreviewQuestionIds[this.normalizeTag(rule.tag)] ?? [];
      const questions = ids
        .map((id) => this.targetDifficultyQuestions.find((question) => question.id === id))
        .filter((question): question is QuestionResponseDto => Boolean(question));

      return { rule, questions };
    });
  }

  private getTagGroup(tag: string): TagGroupSummary | undefined {
    const normalizedTag = this.normalizeTag(tag);
    return this.tagGroups.find((group) => group.key === normalizedTag);
  }

  private getAvailableQuestionCountForRule(tag: string): number {
    const normalizedTag = this.normalizeTag(tag);
    const usedIds = new Set(this.selectedQuestions.map((question) => question.id));

    for (const rule of this.randomTagRules) {
      if (this.normalizeTag(rule.tag) === normalizedTag) {
        break;
      }

      this.getQuestionsByTag(rule.tag)
        .filter((question) => !usedIds.has(question.id))
        .slice(0, rule.count)
        .forEach((question) => usedIds.add(question.id));
    }

    return this.getQuestionsByTag(tag).filter((question) => !usedIds.has(question.id)).length;
  }

  private getQuestionsByTag(tag: string): QuestionResponseDto[] {
    const normalizedTag = this.normalizeTag(tag);
    return this.targetDifficultyQuestions.filter((question) =>
      this.getUniqueTags(question).some((questionTag) => this.normalizeTag(questionTag) === normalizedTag)
    );
  }

  private getUniqueTags(question: QuestionResponseDto): string[] {
    return Array.from(new Set((question.tags ?? []).map((tag) => tag.trim()).filter((tag) => tag.length > 0)));
  }

  private normalizeTag(value: string): string {
    return value.trim().toLocaleLowerCase();
  }

  private shuffleQuestions(questions: QuestionResponseDto[]): QuestionResponseDto[] {
    const shuffled = [...questions];
    for (let i = shuffled.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
    }

    return shuffled;
  }
}
