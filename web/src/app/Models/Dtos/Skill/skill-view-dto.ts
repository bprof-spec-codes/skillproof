import { AssessmentViewDto } from "../Assesment/AssessmentViewDto";

export class SkillViewDto {
    id: string = '';
    name: string = '';
    assessments: AssessmentViewDto[] | null = null;
}