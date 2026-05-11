import { Injectable } from '@angular/core';
import { UserTestsDto } from '../Models/Dtos/User/userTests-dto';
import { ProfileView } from '../components/profile-view/profile-view';

@Injectable({
  providedIn: 'root',
})
export class FeedbackShared {
  selectedQuestion: UserTestsDto | null = null;
  selectedUser: ProfileView | null = null;
  jobId: string | null = null

  constructor(){}
}
