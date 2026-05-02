import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManualFeedback } from './manual-feedback';

describe('ManualFeedback', () => {
  let component: ManualFeedback;
  let fixture: ComponentFixture<ManualFeedback>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ManualFeedback],
    }).compileComponents();

    fixture = TestBed.createComponent(ManualFeedback);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
