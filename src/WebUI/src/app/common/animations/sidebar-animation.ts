import { trigger, style, transition, animate, state } from '@angular/animations';

export const SidebarAnimations = {
    showHideSidebar: trigger('openClose', [
        transition(':enter', [
            style({transform: 'translateX(-100%)'}),
            animate('0.2s', style({transform: 'translateX(0%)'}),)]),
        transition(':leave', [
            animate('0.2s', style({transform: 'translateX(-100%)'}),)]),
        ])
}

export const BoardAnimations = {
    moveBoard: trigger('moveBoard', [
        state('full-page', style({ marginLeft: '0px' })),
        state('sidebar-view-margin', style({ marginLeft: '225px' })),
        state('sidebar-edit-margin', style({ marginLeft: '210px' })),
        transition('full-page <=> sidebar-view-margin, full-page <=> sidebar-edit-margin', 
             animate('0.2s')), 
    ])
}