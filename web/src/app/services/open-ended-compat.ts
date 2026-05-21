import { QuestionResponseDto } from '../Models/Dtos/Question/question-response-dto';
import { CandidateAssessmentDto } from '../Models/Dtos/Test/candidate-assessment-dto';
import { CreateQuestionRequestDto } from '../Models/Dtos/Question/create-question-request-dto';
import { UpdateQuestionRequestDto } from '../Models/Dtos/Question/update-question-request-dto';
import { CandidateQuestionDto } from '../Models/Dtos/Test/candidate-question-dto';
import { DifficultyLevel } from '../Models/Enums/DifficultyLevel';

export function normalizeQuestionResponse(question: QuestionResponseDto): QuestionResponseDto {
  const openEnded = question.openEnded ?? question.fillInTheBlank;
  return {
    ...question,
    difficulty: normalizeDifficultyLevel(question.difficulty),
    tags: normalizeTags(question.tags),
    openEnded,
    fillInTheBlank: question.fillInTheBlank ?? openEnded,
  };
}

export function normalizeQuestionResponses(questions: QuestionResponseDto[]): QuestionResponseDto[] {
  return questions.map((question) => normalizeQuestionResponse(question));
}

export function normalizeCandidateAssessment(
  assessment: CandidateAssessmentDto | null
): CandidateAssessmentDto | null {
  if (!assessment) {
    return null;
  }

  return {
    ...assessment,
    questions: assessment.questions.map((question) => normalizeCandidateQuestion(question)),
  };
}

export function withLegacyOpenEndedWrite<T extends CreateQuestionRequestDto | UpdateQuestionRequestDto>(
  dto: T
): T {
  if (!dto.openEnded || dto.fillInTheBlank) {
    return dto;
  }

  return {
    ...dto,
    fillInTheBlank: dto.openEnded,
  };
}

function normalizeCandidateQuestion(question: CandidateQuestionDto): CandidateQuestionDto {
  const openEnded = question.openEnded ?? question.fillInTheBlank;
  return {
    ...question,
    openEnded,
    fillInTheBlank: question.fillInTheBlank ?? openEnded,
  };
}

function normalizeDifficultyLevel(value: DifficultyLevel | string | number): DifficultyLevel {
  if (typeof value === 'number' && DifficultyLevel[value] !== undefined) {
    return value as DifficultyLevel;
  }

  if (typeof value === 'string') {
    const mapped = DifficultyLevel[value as keyof typeof DifficultyLevel];
    if (typeof mapped === 'number') {
      return mapped as DifficultyLevel;
    }
  }

  return DifficultyLevel.Junior;
}

function normalizeTags(value: unknown): string[] {
  if (!Array.isArray(value)) {
    return [];
  }

  return value.map((tag) => String(tag).trim()).filter((tag) => tag.length > 0);
}
