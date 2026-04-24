import { EmploymentType } from "../../Enums/EmploymentType"

export class JobCreateDto{
    companyId: string = ""
    title: string = ""
    description: string = ""
    employmentType: EmploymentType | null = null
    location: string = ""
    tags: string[] = []
    //companyName: string = ""
}