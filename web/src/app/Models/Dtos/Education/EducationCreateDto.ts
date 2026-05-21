export class EducationCreateDto {
    school: string = '';
    degree: string = '';
    fieldOfStudy: string = '';
    startDate: string = '';
    endDate?: string | null = null;
    description: string = '';
}