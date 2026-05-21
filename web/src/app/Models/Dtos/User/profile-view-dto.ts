import { EducationViewDto } from "../Education/EducationViewDto";
import { SkillViewDto } from "../Skill/skill-view-dto";
import { BadgeDto } from "./badge-dto";
import { ExperienceViewDto } from "../Experience/ExperienceViewDto";

export interface ProfileViewDto {
  id: string;
  fullName: string;
  email: string;
  headline: string;
  bio: string;
  image: string;
  companyId?: string;
  savedJobIds: string[];
  skills: SkillViewDto[];
  badges: BadgeDto[];
  appliedJobIds: string[];
  education: EducationViewDto[];
  userExperience: ExperienceViewDto[];
}
