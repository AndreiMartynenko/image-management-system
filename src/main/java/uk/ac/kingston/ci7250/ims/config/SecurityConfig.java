package uk.ac.kingston.ci7250.ims.config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.annotation.Order;
import org.springframework.security.config.Customizer;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.core.userdetails.User;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.provisioning.InMemoryUserDetailsManager;
import org.springframework.security.web.util.matcher.AntPathRequestMatcher;
import org.springframework.security.web.SecurityFilterChain;

@Configuration
public class SecurityConfig {

    @Bean
    @Order(1)
    public SecurityFilterChain apiSecurityFilterChain(HttpSecurity http) throws Exception {
        return http
                .securityMatcher(new AntPathRequestMatcher("/api/**"))
                .csrf(csrf -> csrf.disable())
                .authorizeHttpRequests(auth -> auth
                        .requestMatchers("/api/admin/**").hasRole("ADMIN")
                        .requestMatchers("/api/staff/**").hasRole("STAFF")
                        .anyRequest().authenticated()
                )
                .httpBasic(Customizer.withDefaults())
                .build();
    }

    @Bean
    @Order(2)
    public SecurityFilterChain uiSecurityFilterChain(HttpSecurity http) throws Exception {
        return http
                .authorizeHttpRequests(auth -> auth
                        .requestMatchers("/", "/error", "/login").permitAll()
                        .requestMatchers("/actuator/health").permitAll()
                        .requestMatchers("/css/**", "/js/**", "/images/**").permitAll()
                        .requestMatchers("/ui/admin/**").hasRole("ADMIN")
                        .requestMatchers("/ui/**").authenticated()
                        .anyRequest().denyAll()
                )
                .formLogin(form -> form
                        .loginPage("/login")
                        .defaultSuccessUrl("/ui", true)
                        .permitAll()
                )
                .logout(logout -> logout
                        .logoutUrl("/logout")
                        .logoutSuccessUrl("/login?logout")
                )
                .build();
    }

    @Bean
    public InMemoryUserDetailsManager userDetailsService() {
        UserDetails radiologist = User.withDefaultPasswordEncoder()
                .username("radiologist")
                .password("password")
                .roles("STAFF")
                .build();

        UserDetails doctor = User.withDefaultPasswordEncoder()
                .username("doctor")
                .password("password")
                .roles("STAFF")
                .build();

        UserDetails admin = User.withDefaultPasswordEncoder()
                .username("admin")
                .password("password")
                .roles("ADMIN", "STAFF")
                .build();

        return new InMemoryUserDetailsManager(radiologist, doctor, admin);
    }
}
