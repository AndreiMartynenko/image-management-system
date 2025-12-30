package uk.ac.kingston.ci7250.ims.web.ui;

import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import uk.ac.kingston.ci7250.ims.service.PatientService;

@Controller
public class UiDashboardController {
    private final PatientService patientService;

    public UiDashboardController(PatientService patientService) {
        this.patientService = patientService;
    }

    @GetMapping("/ui")
    public String dashboard(Model model) {
        model.addAttribute("patientCount", patientService.count());
        return "ui/dashboard";
    }
}
