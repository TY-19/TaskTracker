import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";
import * as moment from 'moment';

export class CustomValidators
{
    static passwordMatchValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            let password = control.get('password');
            let passwordConfirm = control.get('passwordConfirm');
            if (password && passwordConfirm
                && password.value !== "" && passwordConfirm.value !== ""
                && password?.value != passwordConfirm?.value) {
                return { passwordMatchError: true };
            }
            return null;
        };
    }

    static boardNameValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const forbidden = !Number.isNaN(control.value) && 
            Number.parseInt(control.value) == control.value;
            return forbidden ? { nameIsNumber: {value: control.value}} : null;
        };
    }

    static dateInTheFutureValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            let deadlineDate = control.get('deadlineDate')?.value;

            let deadline : moment.Moment = moment.isMoment(deadlineDate)
              ? moment(deadlineDate)
              : moment(deadlineDate, 'YYYY-MM-DDTHH:mm:ss');
            let deadlineDateOnly = new Date(deadline.year(), deadline.month(), deadline.date());

            let deadlineTime = control.get('deadlineTime')?.value;
            let timeMoment = moment(deadlineTime, 'HH:mm');

            deadlineDateOnly.setHours(timeMoment.hours());
            deadlineDateOnly.setMinutes(timeMoment.minutes());

            let dateNow = moment(Date.now()).toDate();
            let isAfter = deadlineDateOnly >= dateNow;
            if (isAfter) {
                return null;
            } else {
                return { dateNotInTheFuture: true };
            }
        };
      }
}