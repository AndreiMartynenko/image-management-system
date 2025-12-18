package uk.ac.kingston.ci7250.ims.web;

import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import uk.ac.kingston.ci7250.ims.domain.ImageRecord;
import uk.ac.kingston.ci7250.ims.service.ImagingService;
import uk.ac.kingston.ci7250.ims.web.dto.UploadImageRequest;

import java.util.List;

@RestController
@RequestMapping("/api/staff/images")
public class ImagingController {
    private final ImagingService imagingService;

    public ImagingController(ImagingService imagingService) {
        this.imagingService = imagingService;
    }

    @PostMapping("/patients/{patientId}")
    @ResponseStatus(HttpStatus.CREATED)
    public ImageRecord upload(@PathVariable long patientId, @Valid @RequestBody UploadImageRequest req) {
        return imagingService.upload(patientId, req.getModality(), req.getDiseaseTag());
    }

    @GetMapping("/patients/{patientId}")
    public List<ImageRecord> list(@PathVariable long patientId) {
        return imagingService.listForPatient(patientId);
    }
}
