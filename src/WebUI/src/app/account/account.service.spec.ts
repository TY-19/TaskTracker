import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AccountService } from './account.service';
import { UserProfile } from '../models/user-profile';
import { environment } from 'src/environments/environment';
import { UserUpdateModel } from '../models/update-models/user-update-model';
import { HttpResponse } from '@angular/common/http';
import { ChangePasswordModel } from '../models/update-models/change-password.model';
import { DummyDataHelper } from 'src/tests/helpers/dummy-data-helper';

describe('AccountService', () => {
  let service: AccountService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ],
      providers: [
        AccountService
      ]
    })
    .compileComponents();

    service = TestBed.inject(AccountService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call GET api/account/profile', () => {
    const dummyUserProfile: UserProfile = DummyDataHelper.getDummyUserProfile();

    service.getUserProfile().subscribe(result => {
      expect(result).toEqual(dummyUserProfile);
    });

    const req = httpMock.expectOne(environment.baseUrl + 'api/account/profile');
    expect(req.request.method).toBe('GET');
    req.flush(dummyUserProfile);
  });

  it('should call PUT api/account/profile', () => {
    const updateUser: UserUpdateModel = {
      lastName: 'New last name'
    };

    service.updateUserProfile(updateUser).subscribe(result => {
      expect(result).toBeInstanceOf(HttpResponse);
      expect(result.status).toBe(204);
    });

    const req = httpMock.expectOne(environment.baseUrl + "api/account/profile");
    expect(req.request.method).toBe('PUT');
    req.flush(null, { status: 204, statusText: 'No content' });
  });

  it('should call PUT api/account/profile/changepassword', () => {
    const changePasswordModel: ChangePasswordModel = {
      oldPassword: 'oldPassword',
      newPassword: 'newPassword'
    };

    service.changePassword(changePasswordModel).subscribe(result => {
      expect(result).toBeInstanceOf(HttpResponse);
      expect(result.status).toBe(204);
    });

    const req = httpMock.expectOne(environment.baseUrl + "api/account/profile/changepassword");
    expect(req.request.method).toBe('PUT');
    req.flush(null, { status: 204, statusText: 'No content' });
  });
});