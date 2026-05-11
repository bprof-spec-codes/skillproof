import { QuestionType } from "../../Enums/QuestionType";

export interface UserTestsDto {
    questionId: string;
    testAnswerId: string;
    score: number;
    questionText: string;
    userResponse: string;
    isInspected: boolean;
    userId: string;
    questionType: QuestionType;
}