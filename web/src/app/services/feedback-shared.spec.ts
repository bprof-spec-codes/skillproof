import { TestBed } from '@angular/core/testing';

import { FeedbackShared } from './feedback-shared';

describe('FeedbackShared', () => {
  let service: FeedbackShared;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FeedbackShared);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
