import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobEdit } from './job-edit';

describe('JobEdit', () => {
  let component: JobEdit;
  let fixture: ComponentFixture<JobEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [JobEdit],
    }).compileComponents();

    fixture = TestBed.createComponent(JobEdit);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
