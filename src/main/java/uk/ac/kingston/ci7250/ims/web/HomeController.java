package uk.ac.kingston.ci7250.ims.web;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.Map;

@RestController
public class HomeController {

    @GetMapping("/")
    public Map<String, String> home() {
        return Map.of(
                "name", "IMS Prototype",
                "health", "/actuator/health",
                "patients", "/api/staff/patients",
                "staff", "/api/admin/staff"
        );
    }
}
