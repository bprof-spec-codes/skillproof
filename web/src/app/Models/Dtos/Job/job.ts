import { EmploymentType } from "../../Enums/EmploymentType"

export class Job{
    id: string = ""
    companyId: string = ""
    title: string = ""
    description: string = ""
    employmentType: EmploymentType | null = null
    location: string = ""
    tags: string = ""
    createdAt: string = ""
}
