import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountComponent } from './account.component';
import { AccountService } from './account.service';
import { DummyDataHelper } from 'src/tests/helpers/dummy-data-helper';
import { of } from 'rxjs';

describe('AccountComponent', () => {
  let component: AccountComponent;
  let fixture: ComponentFixture<AccountComponent>;
  let accountServiceMock: Partial<AccountService>;

  beforeEach(async () => {
    accountServiceMock = {
      getUserProfile: jasmine.createSpy('getUserProfile')
        .and.returnValue(of(DummyDataHelper.getDummyUserProfile())),
      updateUserProfile: jasmine.createSpy('updateUserProfile')
        .and.returnValue(of(DummyDataHelper.getDummyNoContentResponse())),
      changePassword: jasmine.createSpy('changePassword')
        .and.returnValue(of(DummyDataHelper.getDummyNoContentResponse())),
    }

    await TestBed.configureTestingModule({
      declarations: [ AccountComponent ],
      providers: [
        { provide: AccountService, useValue: accountServiceMock }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load profile during ngOninit', () => {
    expect(component.profile).toBeTruthy();
  });

  it('should initialize form during ngOninit', () => {
    expect(component.form).toBeTruthy();
  });

  it('should update profile if form is valid', () => {
    component.form.patchValue(DummyDataHelper.getDummyUserProfile());
    component.onSubmit();
    expect(accountServiceMock.updateUserProfile).toHaveBeenCalledTimes(1);
  });

  it('should not update profile if form is invalid', () => {
    component.form.patchValue({email: 'invalid.email'});
    component.onSubmit();
    expect(accountServiceMock.updateUserProfile).toHaveBeenCalledTimes(0);
  });

  it('should display error if provided value is invalud', () => {
    component.form.patchValue({ email: 'invalid.email' });
    component.onSubmit();
    fixture.detectChanges();
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('.small-warn-text')).toBeTruthy();
  });
});
