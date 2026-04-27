import { EmploymentType } from "../../Enums/EmploymentType"
import { AssessmentViewDto } from '../Assesment/AssessmentViewDto';

export class JobViewDto {
  id: string = '';
  companyId: string = '';
  title: string = '';
  description: string = '';
  employmentType: EmploymentType | null = null;
  shortDescription = "";
  EmploymentType: EmploymentType | null = null;
  location: string = '';
  tags: string[] = [];
  createdAt: string = '';
  assessments: AssessmentViewDto[] = [];
  assessmentIds: string[] = [];
}
