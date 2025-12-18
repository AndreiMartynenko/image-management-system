package uk.ac.kingston.ci7250.ims.web;

import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import uk.ac.kingston.ci7250.ims.domain.WorkflowTask;
import uk.ac.kingston.ci7250.ims.service.WorkflowService;
import uk.ac.kingston.ci7250.ims.web.dto.AddTaskRequest;

import java.util.List;

@RestController
@RequestMapping("/api/staff/workflow")
public class WorkflowController {
    private final WorkflowService workflowService;

    public WorkflowController(WorkflowService workflowService) {
        this.workflowService = workflowService;
    }

    @PostMapping("/patients/{patientId}/tasks")
    @ResponseStatus(HttpStatus.CREATED)
    public WorkflowTask addTask(@PathVariable long patientId, @Valid @RequestBody AddTaskRequest req) {
        return workflowService.addTask(patientId, req.getType(), req.getCost(), req.getPerformedBy());
    }

    @GetMapping("/patients/{patientId}/tasks")
    public List<WorkflowTask> listTasks(@PathVariable long patientId) {
        return workflowService.listTasksForPatient(patientId);
    }
}
