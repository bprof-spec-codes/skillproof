import { EmploymentType } from "../../Enums/EmploymentType"

export class JobViewDto{
    id: string = ""
    companyId: string = ""
    title: string = ""
    description: string = ""
    EmploymentType: EmploymentType | null = null
    location: string = ""
    tags: string[] = []
    createdAt: string = ""
    //companyName: string = ""
}
