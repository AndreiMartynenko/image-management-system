package uk.ac.kingston.ci7250.ims.web.ui;

import jakarta.validation.Valid;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import uk.ac.kingston.ci7250.ims.service.BillingService;
import uk.ac.kingston.ci7250.ims.service.ImagingService;
import uk.ac.kingston.ci7250.ims.service.PatientService;
import uk.ac.kingston.ci7250.ims.service.WorkflowService;
import uk.ac.kingston.ci7250.ims.web.dto.AddTaskRequest;
import uk.ac.kingston.ci7250.ims.web.dto.UploadImageRequest;

@Controller
@RequestMapping("/ui/patients/{patientId}")
public class UiPatientDetailController {
    private final PatientService patientService;
    private final ImagingService imagingService;
    private final WorkflowService workflowService;
    private final BillingService billingService;

    public UiPatientDetailController(PatientService patientService,
                                    ImagingService imagingService,
                                    WorkflowService workflowService,
                                    BillingService billingService) {
        this.patientService = patientService;
        this.imagingService = imagingService;
        this.workflowService = workflowService;
        this.billingService = billingService;
    }

    @GetMapping
    public String details(@PathVariable long patientId, Model model) {
        model.addAttribute("patient", patientService.get(patientId));
        model.addAttribute("images", imagingService.listForPatient(patientId));
        model.addAttribute("tasks", workflowService.listTasksForPatient(patientId));
        model.addAttribute("totalCost", billingService.totalCostForPatient(patientId));

        if (!model.containsAttribute("uploadImageRequest")) {
            model.addAttribute("uploadImageRequest", new UploadImageRequest());
        }
        if (!model.containsAttribute("addTaskRequest")) {
            model.addAttribute("addTaskRequest", new AddTaskRequest());
        }

        return "ui/patient-detail";
    }

    @PostMapping("/images")
    public String uploadImage(@PathVariable long patientId,
                              @Valid @ModelAttribute UploadImageRequest uploadImageRequest,
                              BindingResult bindingResult,
                              Model model) {
        if (bindingResult.hasErrors()) {
            return details(patientId, model);
        }

        imagingService.upload(patientId, uploadImageRequest.getModality(), uploadImageRequest.getDiseaseTag());
        return "redirect:/ui/patients/" + patientId;
    }

    @PostMapping("/tasks")
    public String addTask(@PathVariable long patientId,
                          @Valid @ModelAttribute AddTaskRequest addTaskRequest,
                          BindingResult bindingResult,
                          Model model) {
        if (bindingResult.hasErrors()) {
            return details(patientId, model);
        }

        workflowService.addTask(patientId, addTaskRequest.getType(), addTaskRequest.getCost(), addTaskRequest.getPerformedBy());
        return "redirect:/ui/patients/" + patientId;
    }
}
