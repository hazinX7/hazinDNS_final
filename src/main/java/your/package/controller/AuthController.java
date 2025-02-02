package your.package.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import your.package.service.AuthService;
import your.package.payload.request.RegisterRequest;
import your.package.payload.response.MessageResponse;

@RestController
@RequestMapping("/api/auth")
public class AuthController {

    @Autowired
    private AuthService authService;

    @PostMapping("/register")
    public ResponseEntity<MessageResponse> register(@RequestBody RegisterRequest request) {
        authService.register(request);
        MessageResponse response = new MessageResponse("Регистрация успешна");
        return ResponseEntity.status(201).body(response);
    }
} 