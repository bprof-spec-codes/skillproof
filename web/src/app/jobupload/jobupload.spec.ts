import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Jobupload } from './jobupload';

describe('Jobupload', () => {
  let component: Jobupload;
  let fixture: ComponentFixture<Jobupload>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Jobupload],
    }).compileComponents();

    fixture = TestBed.createComponent(Jobupload);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
