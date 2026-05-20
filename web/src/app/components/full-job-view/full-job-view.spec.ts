import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FullJobView } from './full-job-view';

describe('FullJobView', () => {
  let component: FullJobView;
  let fixture: ComponentFixture<FullJobView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FullJobView],
    }).compileComponents();

    fixture = TestBed.createComponent(FullJobView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
