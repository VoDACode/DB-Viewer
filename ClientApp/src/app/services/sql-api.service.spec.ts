import { TestBed } from '@angular/core/testing';

import { SqlApiService } from './sql-api.service';

describe('SqlApiService', () => {
  let service: SqlApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SqlApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
