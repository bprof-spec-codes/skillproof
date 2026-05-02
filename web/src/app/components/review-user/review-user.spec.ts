import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReviewUser } from './review-user';

describe('ReviewUser', () => {
  let component: ReviewUser;
  let fixture: ComponentFixture<ReviewUser>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ReviewUser],
    }).compileComponents();

    fixture = TestBed.createComponent(ReviewUser);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
