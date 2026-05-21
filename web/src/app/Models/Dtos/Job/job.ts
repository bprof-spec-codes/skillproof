import { EmploymentType } from '../../Enums/EmploymentType';

export class Job {
  id: string = '';
  companyId: string = '';
  title: string = '';
  description: string = '';
  employmentType: EmploymentType | null = null;
  shortDescription: string = '';
  location: string = '';
  salary: number | null = null;
  tags: string = '';
  createdAt: string = '';
  assessments?: any[] = [];
  assessmentIds?: string[] = [];
}
