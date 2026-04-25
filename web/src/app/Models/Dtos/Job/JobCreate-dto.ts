import { EmploymentType } from "../../Enums/EmploymentType"

export class JobCreateDto{
    companyId: string = ""
    title: string = ""
    description: string = ""
    shortDescription = ""
    EmploymentType: EmploymentType | null = null
    location: string = ""
    tags: string[] = []
    //companyName: string = ""
}