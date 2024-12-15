import { TestBed } from '@angular/core/testing';

import { CustomvalidatorService } from './customvalidator.service';

describe('CustomvalidatorService', () => {
  let service: CustomvalidatorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CustomvalidatorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
