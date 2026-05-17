import { TestAnswerSubmitDto } from "./test-answer-submit-dto";

export class TestSubmitSkillDto
{
    skillId: string ='';
    answers: TestAnswerSubmitDto[] | null = null;
}