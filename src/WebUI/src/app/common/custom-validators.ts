import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

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
}