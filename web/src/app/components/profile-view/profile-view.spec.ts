import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileView } from './profile-view';

describe('ProfileView', () => {
  let component: ProfileView;
  let fixture: ComponentFixture<ProfileView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProfileView],
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileView);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
