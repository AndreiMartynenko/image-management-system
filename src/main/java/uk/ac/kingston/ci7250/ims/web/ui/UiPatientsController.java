package uk.ac.kingston.ci7250.ims.web.ui;

import jakarta.validation.Valid;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import uk.ac.kingston.ci7250.ims.service.PatientService;
import uk.ac.kingston.ci7250.ims.web.dto.CreatePatientRequest;

@Controller
@RequestMapping("/ui/patients")
public class UiPatientsController {
    private final PatientService patientService;

    public UiPatientsController(PatientService patientService) {
        this.patientService = patientService;
    }

    @GetMapping
    public String list(Model model) {
        model.addAttribute("patients", patientService.list());
        if (!model.containsAttribute("createPatientRequest")) {
            model.addAttribute("createPatientRequest", new CreatePatientRequest());
        }
        return "ui/patients";
    }

    @PostMapping
    public String create(@Valid @ModelAttribute CreatePatientRequest createPatientRequest,
                         BindingResult bindingResult,
                         Model model) {
        if (bindingResult.hasErrors()) {
            model.addAttribute("patients", patientService.list());
            return "ui/patients";
        }

        patientService.create(
                createPatientRequest.getName(),
                createPatientRequest.getAddress(),
                createPatientRequest.getConditions(),
                createPatientRequest.getDiagnosis()
        );

        return "redirect:/ui/patients";
    }
}
