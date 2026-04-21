import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssessmentCreate } from './assessment-create';

describe('AssessmentCreate', () => {
  let component: AssessmentCreate;
  let fixture: ComponentFixture<AssessmentCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AssessmentCreate],
    }).compileComponents();

    fixture = TestBed.createComponent(AssessmentCreate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
