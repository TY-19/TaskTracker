import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'tt-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  test!: any;
  
  constructor(private http: HttpClient,
    private authService: AuthService) {

  }

  ngOnInit(): void {
    this.authService.init();
  }

  testBackEndConnection() {
    this.http.get("/api/test").subscribe(response => this.test = response);
  }
}
