import { TestBed } from '@angular/core/testing';

import { Assesmentservice } from './assesmentservice';

describe('Assesmentservice', () => {
  let service: Assesmentservice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Assesmentservice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
