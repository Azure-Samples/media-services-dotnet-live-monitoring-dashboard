import { TestBed, async, inject } from '@angular/core/testing';

import { EnsureAuthenticatedGuard } from './ensure-authenticated.guard';

describe('EnsureAuthenticatedGuard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EnsureAuthenticatedGuard]
    });
  });

  it('should ...', inject([EnsureAuthenticatedGuard], (guard: EnsureAuthenticatedGuard) => {
    expect(guard).toBeTruthy();
  }));
});
