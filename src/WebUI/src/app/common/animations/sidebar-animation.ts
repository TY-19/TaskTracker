import { trigger, style, transition, animate } from '@angular/animations';

export const SidebarAnimations = {
    animeTrigger: trigger('openClose', [
        transition(':enter', [
            style({transform: 'translateX(-100%)'}),
            animate('0.2s', style({transform: 'translateX(0%)'}),)]),
        transition(':leave', [
            animate('0.2s', style({transform: 'translateX(-100%)'}),)]),
        ])
}