import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-header-menu',
  templateUrl: './header-menu.component.html',
  styleUrls: ['./header-menu.component.css']
})
export class HeaderMenuComponent implements OnInit {
  isAuthenticated: boolean = false;
  username: string | null | undefined;
  
  constructor(private authService: AuthService, private cdr: ChangeDetectorRef, private router: Router) { }

  ngOnInit(): void {
    this.authService.isAuthenticated().subscribe((authState: boolean) => {
      this.isAuthenticated = authState;
      this.cdr.detectChanges() // Manually trigget change detection.
    });

    this.authService.getUsername().subscribe((username: string | null | undefined) => {
      this.username = username;
      this.cdr.detectChanges() // Manually trigget change detection.
    });
  }

  signOut() {
    this.authService.signOut();
    this.router.navigate(['/home']);
  }
}
