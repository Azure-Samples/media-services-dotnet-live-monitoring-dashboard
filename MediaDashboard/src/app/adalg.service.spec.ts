import { TestBed, inject } from '@angular/core/testing';

import { AdalgService } from './adalg.service';

describe('AdalgService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AdalgService]
    });
  });

  it('should be created', inject([AdalgService], (service: AdalgService) => {
    expect(service).toBeTruthy();
  }));
});
