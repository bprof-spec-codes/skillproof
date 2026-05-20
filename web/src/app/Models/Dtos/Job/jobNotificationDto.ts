import { JobApplicationStatus } from "../../Enums/Status";

export class JobNotificationDto {
    id: string = '';
    jobTitle: string = '';
    status: JobApplicationStatus = JobApplicationStatus.Submitted;
}