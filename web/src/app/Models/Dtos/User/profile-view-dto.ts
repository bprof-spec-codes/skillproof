export interface ProfileViewDto {
  id: string;
  fullName: string;
  email: string;
  headline: string;
  bio: string;
  image: string;
  companyId?: string;
  savedJobIds: string[];
  skills: string[] | null;
}
