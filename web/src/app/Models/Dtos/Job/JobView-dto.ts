import { EmploymentType } from "../../Enums/EmploymentType"
import { QuestionResponseDto } from '../Question/question-response-dto';

export class JobViewDto {
  id: string = '';
  companyId: string = '';
  title: string = '';
  description: string = '';
  EmploymentType: EmploymentType | null = null;
  location: string = '';
  tags: string[] = [];
  createdAt: string = '';
  questions: QuestionResponseDto[] = [];
  //companyName: string = ""
}
