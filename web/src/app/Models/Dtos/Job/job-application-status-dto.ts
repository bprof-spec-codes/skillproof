import { JobApplicationStatus } from "../../Enums/Status";
export class JobApplicationStatusDto {
    jobApplicationStatus: JobApplicationStatus = JobApplicationStatus.Withdrawn
    userId: string = ""
}
