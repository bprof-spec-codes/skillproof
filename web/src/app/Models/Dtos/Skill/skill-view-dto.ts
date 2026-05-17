import { AssessmentViewDto } from "../Assesment/AssessmentViewDto";

export class SkillViewDto {
    id: string = '';
    name: string = '';
    assesments: AssessmentViewDto[] | null = null;
}