import { TestBed, async, inject } from '@angular/core/testing';

import { OauthCallbackGuard } from './oauth-callback.guard';

describe('OauthCallbackGuard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [OauthCallbackGuard]
    });
  });

  it('should ...', inject([OauthCallbackGuard], (guard: OauthCallbackGuard) => {
    expect(guard).toBeTruthy();
  }));
});
