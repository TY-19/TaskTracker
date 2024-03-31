import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegistrationComponent } from './registration.component';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { DummyDataHelper } from 'src/tests/helpers/dummy-data-helper';
import { Subject } from 'rxjs';
import { RegistrationResult } from 'src/app/models/registration-result';

describe('RegistrationComponent', () => {
  let component: RegistrationComponent;
  let fixture: ComponentFixture<RegistrationComponent>;
  let authServiceMock: jasmine.SpyObj<AuthService>;
  let routerMock: Partial<Router>;

  beforeEach(async () => {
    authServiceMock = jasmine.createSpyObj('authService', ['registration']);
    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      declarations: [ RegistrationComponent ],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock}
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegistrationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form during ngOninit', () => {
    expect(component.form).toBeTruthy();
  });

  it('should call registration if form is valid', () => {
    const response = DummyDataHelper.getDummyRegistrationSuccessResult();
    const subject = new Subject<RegistrationResult>();
    authServiceMock.registration.and.returnValue(subject.asObservable());

    component.form.patchValue(DummyDataHelper.getDummyRegistrationRequest());
    component.form.patchValue({ passwordConfirm: "password" });
    component.onSubmit();
    subject.next(response);

    expect(authServiceMock.registration).toHaveBeenCalledTimes(1);
  });

  it('should not call login if form is invalid', () => {
    component.form.patchValue({ nameOrEmail: 'invalidForm'});
    component.onSubmit();
    expect(authServiceMock.registration).toHaveBeenCalledTimes(0);
  });

  it('form should be invalid if passwords doesn\'t match', () => {
    const response = DummyDataHelper.getDummyRegistrationSuccessResult();
    const subject = new Subject<RegistrationResult>();
    authServiceMock.registration.and.returnValue(subject.asObservable());

    component.form.patchValue(DummyDataHelper.getDummyRegistrationRequest());
    component.form.patchValue({ passwordConfirm: "wrongPassword" });
    component.onSubmit();
    subject.next(response);

    expect(authServiceMock.registration).toHaveBeenCalledTimes(0);
  });

  it('should display backend validation error message', () => {
    const response = DummyDataHelper.getDummyRegistrationFailResult();
    const subject = new Subject<RegistrationResult>();
    authServiceMock.registration.and.returnValue(subject.asObservable());

    component.form.patchValue(DummyDataHelper.getDummyRegistrationRequest());
    component.form.patchValue({ passwordConfirm: "password" });
    component.onSubmit();
    subject.next(response);

    expect(authServiceMock.registration).toHaveBeenCalledTimes(1);
    expect(component.registrationResult?.success).toBeFalse();
    
    fixture.detectChanges();
    const compiled = fixture.nativeElement;
    const elem = compiled.querySelector('mat-error') as HTMLElement;
    expect(elem.innerHTML.includes(response.message));
  });
});
