export class EducationViewDto {
    id: string = '';
    school: string = '';
    degree: string = '';
    fieldOfStudy: string = '';
    startDate: string = '';
    endDate?: string | null = null;
    isOngoing: boolean = false;
    description: string = '';
}