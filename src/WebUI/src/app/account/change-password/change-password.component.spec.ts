import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangePasswordComponent } from './change-password.component';
import { AccountService } from '../account.service';
import { of } from 'rxjs';
import { DummyDataHelper } from 'src/tests/helpers/dummy-data-helper';
import { Router } from '@angular/router';

describe('ChangePasswordComponent', () => {
  let component: ChangePasswordComponent;
  let fixture: ComponentFixture<ChangePasswordComponent>;
  let accountServiceMock: Partial<AccountService>;

  beforeEach(async () => {
    accountServiceMock = {
      changePassword: jasmine.createSpy('changePassword')
        .and.returnValue(of(DummyDataHelper.getDummyNoContentResponse())),
    }
    const routerStub = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      declarations: [ ChangePasswordComponent ],
      providers: [
        { provide: AccountService, useValue: accountServiceMock },
        { provide: Router, useValue: routerStub }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangePasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form during ngOninit', () => {
    expect(component.form).toBeTruthy();
  });

  it('should change password if form is valid', () => {
    component.form.patchValue(DummyDataHelper.getDummyChangePasswordModel());
    component.onSubmit();
    expect(accountServiceMock.changePassword).toHaveBeenCalledTimes(1);
  });

  it('should not update profile if form is invalid', () => {
    component.form.patchValue({oldPassword: 'oldPassword', password: "0", passwordConfirm: "1"});
    component.onSubmit();
    expect(accountServiceMock.changePassword).toHaveBeenCalledTimes(0);
  });

  it('should display error if provided value is invalud', () => {
    component.form.patchValue({oldPassword: 'oldPassword', newPassword: "0", passwordConfirm: "1"});
    component.onSubmit();
    fixture.detectChanges();
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('.small-warn-text')).toBeTruthy();
  });
});
