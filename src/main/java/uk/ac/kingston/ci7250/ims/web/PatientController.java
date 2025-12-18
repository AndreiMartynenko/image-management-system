package uk.ac.kingston.ci7250.ims.web;

import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import uk.ac.kingston.ci7250.ims.domain.Patient;
import uk.ac.kingston.ci7250.ims.service.BillingService;
import uk.ac.kingston.ci7250.ims.service.PatientService;
import uk.ac.kingston.ci7250.ims.web.dto.CreatePatientRequest;

import java.math.BigDecimal;
import java.util.List;
import java.util.Map;

@RestController
@RequestMapping("/api/staff/patients")
public class PatientController {
    private final PatientService patientService;
    private final BillingService billingService;

    public PatientController(PatientService patientService, BillingService billingService) {
        this.patientService = patientService;
        this.billingService = billingService;
    }

    @PostMapping
    @ResponseStatus(HttpStatus.CREATED)
    public Patient create(@Valid @RequestBody CreatePatientRequest req) {
        return patientService.create(req.getName(), req.getAddress(), req.getConditions(), req.getDiagnosis());
    }

    @GetMapping
    public List<Patient> list() {
        return patientService.list();
    }

    @GetMapping("/{id}")
    public Patient get(@PathVariable long id) {
        return patientService.get(id);
    }

    @GetMapping("/{id}/total-cost")
    public Map<String, BigDecimal> totalCost(@PathVariable long id) {
        return Map.of("patientId", BigDecimal.valueOf(id), "totalCost", billingService.totalCostForPatient(id));
    }

    @DeleteMapping("/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    public void delete(@PathVariable long id) {
        patientService.delete(id);
    }
}
