import { AssessmentViewDto } from "../Assesment/AssessmentViewDto";

export class SkillCreateDto {
    name: string = '';
    assesments: AssessmentViewDto[] | null = null;
}