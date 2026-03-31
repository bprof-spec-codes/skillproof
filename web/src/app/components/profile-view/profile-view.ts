import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../services/profile-service';
import { Observable } from 'rxjs';
import { ProfileViewDto } from '../../Models/User/profile-view-dto';

@Component({
  selector: 'app-profile-view',
  standalone: false,
  templateUrl: './profile-view.html',
  styleUrl: './profile-view.scss',
})
export class ProfileView implements OnInit {

  constructor(private profileService: ProfileService) {}

  profile$!: Observable<ProfileViewDto | null>;
  

  ngOnInit(): void {
    this.profile$ = this.profileService.currentProfile$;
  }

}
